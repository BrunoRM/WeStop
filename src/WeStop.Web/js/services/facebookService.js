angular.module('WeStop').factory('facebookService', ['$q', function($q) {

    return {

        connect: function () {
            var deferred = $q.defer();
            FB.getLoginStatus(function(resp) {
                if (resp.status !== 'connected') {
                    FB.login(function (resp) {
                        if (resp.status === 'connected') {
                            deferred.resolve(resp);
                        } else {
                            deferred.reject(resp);
                        }
                    }, { scope: 'public_profile' });
                } else {
                    deferred.resolve(resp);
                }
            });

            return deferred.promise;
        },

        me: function (fields) {
            var deferred = $q.defer();
            FB.api('/me', {
                fields: fields
            }, function (resp) {
                if (!resp || resp.error) {
                    deferred.reject('Error occured');
                } else {
                    deferred.resolve(resp);
                }
            });
            return deferred.promise;
        },

        getProfilePic: function (userId) {
            var deferred = $q.defer();
            FB.api(userId + '/picture?redirect=false&height=200&width=200', function(resp) {
                deferred.resolve(resp.data.url);
            });

            return deferred.promise;
        }
    };

}]);