angular.module('WeStop').controller('createUserController', ['$scope', 'uuid', '$rootScope', '$user', '$location', 'facebookService', 'googleService', function ($scope, uuid, $rootScope, $user, $location, facebookService, googleService) {
    
    $scope.user = { name: '', image: '/images/default-user.jpg' };

    $scope.createUser = () => {
        createUserAndRedirectToLobby($scope.user.name, $scope.user.image);
    };

    $scope.createWithFacebook = () => {
        facebookService.connect().then(function(resp) {
            facebookService.me('first_name').then(function(resp) {
                $scope.user.name = resp.first_name;
                facebookService.getProfilePic(resp.id).then(function(resp) {
                    $scope.user.image = resp;
                    createUserAndRedirectToLobby($scope.user.name, $scope.user.image);
                });
            }, function (error) {
                console.log(error);
            });
        });
    };

    $scope.createWithGoogle = () => {
        googleService.getUserNameAndProfilePic().then((resp) => {
            createUserAndRedirectToLobby(resp.userName, resp.imageUri);
        }, function (error) {

        });
    };

    function createUserAndRedirectToLobby(userName, imageUri) {
        $user.create({
            id: uuid.v4(),
            userName: userName,
            imageUri: imageUri
        });

        $location.path('/lobby');
    }
}]);