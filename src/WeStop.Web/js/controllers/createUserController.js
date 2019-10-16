angular.module('WeStop').controller('createUserController', ['$scope', 'uuid', '$rootScope', '$user', '$location', 'facebookService', function ($scope, uuid, $rootScope, $user, $location, facebookService) {
    
    $scope.user = { name: '', image: '/images/default-user.jpg' };

    $scope.createUser = () => {
        if ($scope.user.name) {
            $user.create({
                id: uuid.v4(),
                userName: $scope.user.name,
                image: $scope.user.image
            });
            
            $location.path('/lobby');
        }
    };

    $scope.createWithFacebook = () => {
        facebookService.getUserNameAndImage().then(function(resp) {
            console.log(resp);
        }, (error) => console.log(error));
    };
}]);