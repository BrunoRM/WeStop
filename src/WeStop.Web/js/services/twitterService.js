angular.module('WeStop').factory('twitterService', ['$q', function ($q) {
    
    return {
        getUserNameAndProfilePic: function () {
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