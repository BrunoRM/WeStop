angular.module('WeStop').controller('lobbyController', ['$scope', '$location', '$rootScope', '$user', '$http', 'API_SETTINGS', 'uuid', function ($scope, $location, $rootScope, $user, $http, API_SETTINGS, uuid) {

    $scope.hasUser = $rootScope.user !== null;
    $scope.newUser = '';

    $scope.createUser = () => {
        if ($scope.newUser) {

            $user.create({
                id: uuid.v4(),
                userName: $scope.newUser
            });
            
            $scope.hasUser = true;
        }
    };

    $http.get(API_SETTINGS.uri + '/games.list').then((resp) => {
        $scope.games = resp.data.games;
    }, (error) => {
        console.error(error);
    });

    $scope.gameDetails = null;
    $scope.showGameDetails = false;

    $scope.details = function(game) {

        $scope.gameDetails = game;
        $scope.showGameDetails = true;

    };

    $scope.backToGames = function () {
        $scope.gameDetails = null;
        $scope.showGameDetails = false;
    };

    $scope.joinGame = () => 
        $location.path('/game/' + $scope.gameDetails.id);

    $scope.newGame = () =>
        $location.path('/game/create');

}]);