using System.Collections;
using System.Collections.Generic;
using MyBase.Observer;
using UnityEngine;

public class TestObserver : EventListener
{
    const string ev1 = "ev1";
    const string ev2 = "ev2";
    const string ev3 = "ev3";
    
    void Start()
    {
        Attach(ev1, nameof(Test1));
        Attach(ev2, nameof(Test2));
        Attach(ev3, nameof(Test3));

        InvokeRepeating(nameof(CallEvent), 5, 10);
    }

    private void CallEvent()
    {
        Notify(ev1, true);
        Notify(ev2, 1);
        Notify(ev3, "e2ewd", 1243124.4334f, 5000, false);
    }

    private void Test1()
    {
        Debug.Log("Test 1 no param");
    }

    private void Test2(string s)
    {
        Debug.Log("Test 2 one param " + s);
    }

    private int Test3(int a, float b, bool c, string d)
    {
        Debug.Log("Test 3 four params " + a + b + c + d);
        return (int)(a + b + (c ? 1 : 0));
    }

    public override void OnEvent(string type, object source, object data = null)
    {
        
    }
}
