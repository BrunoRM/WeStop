angular.module('WeStop').factory('httpInterceptor', ['$rootScope', '$q', function ($rootScope, $q){
    var loadingCount = 0;
    return {
        
        request: function (request) {

            $rootScope.isLoading = true;
            loadingCount++;
            return request;
        },

        requestError: function (error) {

            if (--loadingCount <= 0)
                $rootScope.isLoading = false;

            return $q.reject(error);
        },

        response: function (response) {

            if (--loadingCount <= 0) {
                $rootScope.isLoading = false;
            }

            return response;
        },

        responseError: function (error) {

            if (--loadingCount <= 0)
                $rootScope.isLoading = false;

            return $q.reject(error);
        }
    };

}]);