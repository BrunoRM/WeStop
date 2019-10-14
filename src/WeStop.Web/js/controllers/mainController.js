angular.module('WeStop').controller('mainController', ['$scope', '$location', '$user', function ($scope, $location, $user) {

    $scope.logout = function () {
        $user.logout(() => $location.path('/'));
    };

}]);