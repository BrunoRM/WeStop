angular.module('WeStop').factory('googleService', ['$q', '$window', function ($q, $window) {

    function buildUser(user) {
        let basicProfile = user.getBasicProfile();

        return {
            userName: basicProfile.getGivenName(),
            imageUri: basicProfile.getImageUrl()
        };
    }
    
    return {
        
        getUserNameAndProfilePic: function () {
            let deferred = $q.defer();

            gapi.load('auth2', function () {

                gapi.auth2.init({
                    client_id: '418166283498-phlpf67p4nitc1gd79s51afl3ruqls2d.apps.googleusercontent.com',
                    fetch_basic_profile: true
                }).then(function (resp) {

                    let currentUser = resp.currentUser.get();

                    if (!currentUser || !currentUser.isSignedIn()) {
                        resp.signIn({
                            prompt: 'select_account'
                        }).then(function (resp) {
                            let user = buildUser(resp);
                            deferred.resolve(user);
                        }, function (error) {
                            deferred.reject(error);
                        });
                    } else {
                        let user = buildUser(currentUser);
                        deferred.resolve(user);
                    }
        
                });
            });

            return deferred.promise;
        }
    };

}]);