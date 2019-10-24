angular.module('WeStop').controller('lobbyController', ['$scope', '$location', '$http', 'API_SETTINGS', '$mdDialog', function ($scope, $location, $http, API_SETTINGS, $mdDialog) {

    $http.get(API_SETTINGS.uri + '/api/games.list').then((resp) => {
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

    $scope.checkGame = () => {
        if ($scope.gameDetails.isPrivate) {

            $mdDialog.show({
                templateUrl: './views/game-password.html',
                escapeToClose: true,
                preserveScope: true
            }).then(function (result) {
                console.log('Confirmado')
                $scope.status = 'You decided to name your dog ' + result + '.';
            }, function () {
            });

            $scope.confirmPassword = () => {
                console.log('caiu')
                $mdDialog.hide();
            }
        }
    };


    $scope.joinGame = () => 
        $location.path('/game/' + $scope.gameDetails.id);

    $scope.newGame = () =>
        $location.path('/game/create');

}]);