angular.module('WeStop').controller('lobbyController', ['$scope', '$location', '$rootScope', '$user', '$http', 'API_SETTINGS', function ($scope, $location, $rootScope, $user, $http, API_SETTINGS) {

    $scope.hasUser = $rootScope.user !== null;
    $scope.newUser = '';

    $scope.createUser = () => {
        if ($scope.newUser) {
            
            $http.post(API_SETTINGS.uri + '/users.create', { userName: $scope.newUser }).then((resp) => {
                $user.create(resp.data.user);
                $scope.hasUser = true;
            }, (error) => {
                console.error(error);
            });

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