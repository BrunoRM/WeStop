angular.module('WeStop').controller('lobbyController', ['$scope', '$location', '$http', 'API_SETTINGS', '$mdDialog', '$rootScope', '$toast', '$hub', function ($scope, $location, $http, API_SETTINGS, $mdDialog, $rootScope, $toast, $hub) {

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
        $http.post(API_SETTINGS.uri + '/api/games.authorize?gameid=' + $scope.gameDetails.id + '&password=' + password, $rootScope.user)
            .then((result) => {
                if (!result.data.ok) {
                    switch (result.data.error) {
                        case 'INCORRECT_PASSWORD':
                            $toast.show('Senha incorreta');
                            break;
                        case 'GAME_NOT_FOUND':
                            $toast.show('Essa partida não existe mais');
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

    
    let hubConnection = $hub.createConnection('/lobby');

    $hub.on(hubConnection, 'game_created', (resp) => {
        $scope.games.push(resp);
    });

    $hub.on(hubConnection, 'player_joined_game', (resp) => {
        let game = getGameWithId(resp);
        game.numberOfPlayers++;
    });

    $hub.on(hubConnection, 'round_started', (resp) => {
        let game = getGameWithId(resp);
        game.currentRoundNumber++;
    });
    
    $hub.on(hubConnection, 'player_left_game', (resp) => {
        let game = getGameWithId(resp);
        game.numberOfPlayers--;
    });

    $hub.on(hubConnection, 'game_finished', (resp) => {
        let game = getGameWithId(resp);
        let indexOfGame = $scope.games.indexOf(game);
        $scope.games.splice(indexOfGame, 1);
    });

    function getGameWithId(id) {
        return $scope.games.find((g) => g.id == id);
    }

    $hub.connect(hubConnection);
}]);