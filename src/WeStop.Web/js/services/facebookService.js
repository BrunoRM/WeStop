angular.module('WeStop').factory('facebookService', ['$q', function($q) {

    return {
        getUserNameAndImage: function () {
            var deferred = $q.defer();
            FB.api('/me', {
                fields: 'first_name profile_pic'
            }, function (response) {
                if (!response || response.error) {
                    deferred.reject('Error occured');
                } else {
                    deferred.resolve(response);
                }
            });
            return deferred.promise;
        }
    }

}]);