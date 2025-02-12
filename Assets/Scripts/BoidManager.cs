using System.Collections;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using static Greyhole.Myid.MyID.MyIDClient;
using Greyhole.Myid.Code;
using System;
using Cysharp.Net.Http;
using System.Net.Http;
using System.Net.WebSockets;

public class BoidManager : MonoBehaviour {

    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    public 
    Boid[] boids;

    private Greyhole.Myid.MyID.MyIDClient client = null;

    Metadata Header(string token = "")
    {
        var headers = new Metadata
        {
            { 
                "Authorization", $"Bearer {token}" 
            }
        };
        return headers;
    }

    void Start () 
    {
        boids = FindObjectsOfType<Boid> ();
        foreach (Boid b in boids) {
            b.Initialize (settings, null);
        }
        
        // AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
        using var handler = new YetAnotherHttpHandler();
        var httpClient = new HttpClient(handler);
        var channel = GrpcChannel.ForAddress(@"https://api-stg.cocopark.fun", new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.SecureSsl,
            HttpClient = httpClient
        });
        client = new Greyhole.Myid.MyID.MyIDClient(channel);
        SigninTelegram(MyCallback);
    }

    private delegate void SigninTelegramCallback(string token, RpcException e);
    private delegate bool SigninTelegramCallback2(string token);

    private void MyCallback(string token, RpcException e)
    {
        Debug.Log($"SigninTelegram token: {token}");
    }

    async void SigninTelegram(SigninTelegramCallback cb = null, SigninTelegramCallback2 cb2 = null)
    {
        var request = new Greyhole.Myid.SignInTelegramRequest
        {
            TelegramId = 20,
            Username = "test",
            FirstName = "test",
            LastName = "test",
            PhotoUrl = "",
        };

        try
        {
            Greyhole.Myid.SignInTelegramReply reply = await client.SignInTelegramAsync(request, Header());
            Debug.Log(reply.TokenInfo);
            cb?.Invoke(reply.TokenInfo.AccessToken, null);
            cb2?.Invoke(reply.TokenInfo.AccessToken);
        }
        catch(RpcException e)
        {
            Debug.LogError($"SigninTelegram failed: {e.Status.StatusCode} - {e.Message} - {e.Status.Detail}");
            Code foo = (Code)e.Status.StatusCode;
            Debug.LogError($"SigninTelegram failed error code convert: {foo}");
            cb?.Invoke("", e);
            cb2?.Invoke("");
        }
    }

    void Update () {
        if (boids != null) {

            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++) {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", boids.Length);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt (numBoids / (float) threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++) {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid ();
            }

            boidBuffer.Release ();
        }
    }

    public struct BoidData {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }
}