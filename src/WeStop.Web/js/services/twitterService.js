angular.module('WeStop').factory('twitterService', ['$q', function ($q) {
    
    const firebaseConfig = {
        apiKey: "AIzaSyAMtxk9YGof82udlAC_LkkB4mgc-JxFmAI",
        authDomain: "westop-1571110414274.firebaseapp.com",
        databaseURL: "https://westop-1571110414274.firebaseio.com",
        projectId: "westop-1571110414274",
        storageBucket: "westop-1571110414274.appspot.com",
        messagingSenderId: "418166283498",
        appId: "1:418166283498:web:7ab90ab027f742f6b80b93"
    };
    
    function initializeFirebaseApp() {
        firebase.initializeApp(firebaseConfig);
    }

    function checkFirebaseIsInitialized() {
        return (!firebase.apps.length) ? false : true;
    }

    return {
        getUserNameAndProfilePic: function () {

            if (!checkFirebaseIsInitialized()) {
                initializeFirebaseApp();
            }

            let provider = new firebase.auth.TwitterAuthProvider();
            
            let deferred = $q.defer();
            firebase.auth().signInWithPopup(provider).then(function(result) {
                
                let user = result.user;
                deferred.resolve({
                    userName: user.displayName.split(' ')[0],
                    imageUri: user.photoURL
                });
                
            }).catch(function(error) {
                deferred.reject(error);
            });

            return deferred.promise;
        }
    };

}]);