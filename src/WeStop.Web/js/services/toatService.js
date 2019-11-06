angular.module('WeStop').factory('$toast', ['$mdToast', function($mdToast) {
    return {
        show: function(message) {
            $mdToast.show(
                $mdToast.simple()
                    .textContent(message)
                    .position('bottom left')
                    .hideDelay(3500)
            );
        }
    };
}]);