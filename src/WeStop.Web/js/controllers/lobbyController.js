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
                preserveScope: true,
                fullscreen: true,
                controller: ['$scope', '$http', 'API_SETTINGS', function ($scope, $http, API_SETTINGS) {

                    $scope.password = '';

                    $scope.confirm = (password) => {
                        $mdDialog.hide(password);
                    };

                    $scope.cancel = () => {
                        $mdDialog.cancel();
                    };
                }]
            }).then(function (password) {
                $http.post(API_SETTINGS.uri + '/api/games.check?gameid=' + $scope.gameDetails.id + '&password=' + password).then((result) => {
                    if (!result.data.ok) {
                        switch (result.data.error) {
                            case 'PASSWORD_INCORRECT':
                                alert('Senha incorreta');
                                break;
                            case 'GAME_FULL':
                                alert('Partida cheia');
                                break;
                            case 'GAME_IN_FINAL_ROUND':
                                alert('Partida não encontrada');
                                break;
                            default:
                                break;
                        }
                    } else {
                        $scope.joinGame();
                    }
                }, () => { });
            }, () => { });
        } else {
            $scope.joinGame();        
        }
    };

    $scope.joinGame = () => 
        $location.path('/game/' + $scope.gameDetails.id);

    $scope.newGame = () =>
        $location.path('/game/create');

}]);