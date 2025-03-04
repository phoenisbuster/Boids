import { initializeApp } from "https://www.gstatic.com/firebasejs/10.9.0/firebase-app.js";
import { getAnalytics, logEvent, setUserProperties, setUserId, setAnalyticsCollectionEnabled } from "https://www.gstatic.com/firebasejs/10.9.0/firebase-analytics.js";
//Firebase cloud messaging
import { getMessaging, getToken } from "https://www.gstatic.com/firebasejs/10.9.0/firebase-messaging.js"

// Firebase Authentication
import { getAuth, signInWithPopup, TwitterAuthProvider } from "https://www.gstatic.com/firebasejs/10.9.0/firebase-auth.js";

const firebaseConfig = {
  apiKey: "your_api_key",
  authDomain: "coco-park-telegram.firebaseapp.com",
  projectId: "coco-park-telegram",
  storageBucket: "coco-park-telegram.appspot.com",
  messagingSenderId: "542915110820",
  appId: "your_app_id",
  measurementId: "your_measurement_id"
};

//FCM-----------------------
const publicVapidKey =
  "BHU72gbUZspoVUkYJN4Ij2A8bzJAatxT-Uxn-QY3-KG1g9MK0T9zX6rxbY3iZ4qJyQTpHB5En3xo4-3YCzQK2OA";

var firebaseApp;
var firebaseAuth;
var firebaseAnalytics;

//FCM-------------------
var swRegistration;
var firebaseMessaging;
var isSubscribed = false;
var getTokenTryCount = 10;
var fcmToken = null;
var fcmMessage = null;
//----------------------

//Firebase Authentication -------------------------------------------------
const provider = new TwitterAuthProvider();
window.signInWithTwitterPopup = function()
{
  return signInWithPopup(firebaseAuth, provider)
    .then((result) => {
      // This gives you a the Twitter OAuth 1.0 Access Token and Secret.
      // You can use these server side with your app's credentials to access the Twitter API.
      const credential = TwitterAuthProvider.credentialFromResult(result);
      const token = credential.accessToken;
      const secret = credential.secret;

      // The signed-in user info.
      const user = result.user;
      // IdP data available using getAdditionalUserInfo(result)
      // ...

      console.log("Sign In Twitter", credential, token, secret, user);
      return {token: token, secret: secret, user: user, error: false};
    }).catch((error) => {
      // Handle Errors here.
      const errorCode = error.code;
      const errorMessage = error.message;
      // The email of the user's account used.
      const email = error.customData.email;
      // The AuthCredential type that was used.
      const credential = TwitterAuthProvider.credentialFromError(error);
      // ...
      console.error("Sign In Twitter Error", errorCode, errorMessage, email, credential);
      return {token: "", secret: "", user: null, error: true};
    });
} 

//FirebaseAnalytics -------------------------------------------------
window.initFirebaseApp = function(config) {
  if(config == null) {
    config = firebaseConfig;
  }
  firebaseApp = initializeApp(config);
  firebaseAnalytics = getAnalytics(firebaseApp);
  setAnalyticsCollectionEnabled(firebaseAnalytics, true);
  firebaseAuth = getAuth(firebaseApp);
  firebaseAuth.languageCode = 'it';
  return firebaseApp;
}

window.logFirebaseEvent = function(event_name, data) {
  logEvent(firebaseAnalytics, event_name, data);
}

window.seFirebasetUserProperties = function(property) {
  setUserProperties(firebaseAnalytics, property);
}

window.setFirebaseUserId = function(userId) {
  setUserId(firebaseAnalytics, userId);
}
//End sFirebaseAnalytics -------------------------------------------------