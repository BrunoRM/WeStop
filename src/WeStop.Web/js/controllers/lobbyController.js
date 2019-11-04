 angular.module('WeStop').controller('lobbyController', ['$scope', '$location', '$http', 'API_SETTINGS', '$mdDialog', '$rootScope', '$mdToast', '$lobby', function ($scope, $location, $http, API_SETTINGS, $mdDialog, $rootScope, $mdToast, $lobby) {

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
                controller: ['$scope', function ($scope) {

                    $scope.password = '';

                    $scope.confirm = (password) => {
                        $mdDialog.hide(password);
                    };

                    $scope.cancel = () => {
                        $mdDialog.cancel();
                    };
                }]
            }).then(function (password) {
                authorize(password);
            }, () => { });
        } else {
            authorize('');
        }
    };

    function authorize(password) {
        $http.post(API_SETTINGS.uri + '/api/games.authorize?gameid=' + $scope.gameDetails.id + '&password=' + password, $rootScope.user).then((result) => {
            if (!result.data.ok) {
                switch (result.data.error) {
                    case 'INCORRECT_PASSWORD':
                        $mdToast.show(
                            $mdToast.simple()
                                .textContent('Senha incorreta')
                                .position('bottom left')
                                .hideDelay(3500)
                        );
                        break;
                    case 'GAME_NOT_FOUND':
                        $mdToast.show(
                            $mdToast.simple()
                                .textContent('Essa partida não existe mais')
                                .position('bottom left')
                                .hideDelay(3500)
                        );
                        break;
                }
            } else {
                $scope.joinGame();
            }
        }, () => { });
    }

    $scope.joinGame = () => 
        $location.path('/game/' + $scope.gameDetails.id);

    $scope.newGame = () =>
        $location.path('/game/create');

    $lobby.on('game_created', (resp) => {
        console.log(resp);
    });

    $lobby.on('player_joined_game', (resp) => {
        console.log(resp);
    });

    $lobby.on('round_started', (resp) => {
        console.log(resp);
    });
    
    $lobby.on('player_left_game', (resp) => {
        console.log(resp);
    });

    $lobby.on('game_finished', (resp) => {
        console.log(resp);
    });
}]);