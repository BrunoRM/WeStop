angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', '$rootScope', function ($routeParams, $scope, $game, $rootScope) {

    function init() {
        $scope.allPlayersReady = false;
        $scope.gameStarted = false;
        $scope.stopCalled = false;
        $scope.roundFinished = false;
        $scope.endGame = false;
        $scope.pontuation = 0;
        $scope.currentRoundTime = 0;
        $scope.currentValidationTime = 0;
        $scope.themeValidations = null;
    };

    function joinGame() {
        $game.invoke("game_join", {
            gameId: $routeParams.id,
            userId: $rootScope.user.id
        });
    };

    function setGame(game) {
        $scope.game = game;
    };
    
    function setPlayer(player) {
        $scope.player = player;
    };

    function setWaitingState() {
        $scope.currentRoundTime = 0;
        $scope.currentValidationTime = 0;
        $scope.roundFinished = true;
        $scope.gameStarted = false;
        $scope.stopCalled = false;
        $scope.currentRound++;
        $scope.sortedLetter = '';
    }

    function startValidationTimeForTheme(themeValidations) {
        $scope.gameStarted = true;
        $scope.stopCalled = true;
        $scope.themeValidations = themeValidations;
        $scope.currentValidationTime = 0;
    }

    function startRound() {
        $scope.gameStarted = true;
        $scope.answers = [];

        for (let i = 0; i < $scope.game.themes.length; i++) {
            $scope.answers.push({
                theme: $scope.game.themes[i],
                answer: ''
            });
        }

        $scope.roundFinished = false;
    }

    function getPlayerRoundPontuation() {
        return $scope.game.scoreBoard.find(playerScore => {
            if (playerScore.playerId === $rootScope.user.id) {
                return playerScore.lastRoundPontuation;
            }
        });
    };

    function checkAllPlayersReady() {

        if ($scope.players.length === 1) return false;

        $scope.allPlayersReady = !$scope.players.some(player => {
            return player.userName !== $scope.player.userName && !player.isReady;
        });
    }

    function refreshCurrentValidationTime(newValue) {
        $scope.currentValidationTime = newValue;
    }

    function finishGame() {
        $scope.endGame = true;
    }

    function cleanThemeValidations() {
        $scope.themeValidations = null;
    }

    function stop() {
        $scope.stopCalled = true;
        $scope.roundTime = 0;
    }

    function refreshGamePontuation() {
        $scope.pontuation = getPlayerRoundPontuation();
    }

    function setWinners(winners) {
        $scope.winners = winners;
    }

    function setPlayers(players) {
        $scope.players = players;
    }

    init();
    joinGame();

    $game.on("im_joined_game", (data) => {
        console.log(data)
        setGame(data.game);
        setPlayer(data.player);
        setPlayers(data.game.players);
        checkAllPlayersReady();
        setWaitingState();

        
    });

    $game.on('im_reconected_game', (resp) => {
        
        setGame(resp.game);
        setPlayer(resp.player);
        setPlayers(resp.game.players);
        startRound();

        switch (resp.game.state) {
            case "InProgress":
                startRound();
                break;
            case "ThemesValidations":
                if (resp.validated) {
                    cleanThemeValidations();
                } else {
                    startValidationTimeForTheme(resp.themeValidations);
                }
                break;
            case "Finished":
                setWinners(resp.winners);
                finishGame();
                refreshGamePontuation();
                break;
            default:
                break;
        }
    });

    $scope.startGame = () => {
        $scope.changeStatus();
        $game.invoke('game_start_round', {
            gameRoomId: $routeParams.id,
            userId: $rootScope.user.id
        });
    };

    $game.on('game_round_started', data => {
        startRound();
    });

    $game.on('player_joined_game', data => {
        
        let player = $scope.players.find((player) => {
            return player.userName == data.player.userName;
        });

        if (!player)
            $scope.players.push(data.player);

        checkAllPlayersReady();
    });

    $scope.changeStatus = () => {

        $game.invoke('player_change_status', {
            gameId: $routeParams.id,
            userId: $rootScope.user.id,
            isReady: !$scope.player.isReady
        });

        var player = $scope.players.find((player) => {
            return player.userName == $scope.player.userName;
        });

        $scope.player.isReady = player.isReady = !$scope.player.isReady;
    };

    $game.on('player_status_changed', resp => {
        let player = $scope.players.find((player) => {
            return player.userName === resp.player.userName;
        });

        player.isReady = resp.player.isReady;
        checkAllPlayersReady();
    });

    $scope.stop = () => {

        $game.invoke('game_stop', {
            gameId: $routeParams.id,
            userId: $rootScope.user.id
        });

    };

    $game.on('game_stop', (resp) => {
        
        if (resp.reason === 'player_call_stop') {
            //alert(resp.userName + ' chamou STOP!');
        }

        stop();
        $game.invoke('send_answers', {
            gameId: $routeParams.id,
            userId: $rootScope.user.id,
            answers: $scope.answers
        });

    });

    $scope.validate = (answersValidations) => {

        let obj = {
            gameId: $routeParams.id,
            userId: $rootScope.user.id,
            validation: answersValidations
        }

        $game.invoke('send_validations', obj);
    };
    
    $game.on('im_send_validations', data => {
        cleanThemeValidations();
    });

    function refreshGameScoreBoard(scoreBoard) {
        $scope.game.scoreBoard = scoreBoard;
    }

    $game.on('game_round_finished', resp => {
        setWaitingState();
        refreshGameScoreBoard(resp.scoreBoard);
        refreshGamePontuation();
        cleanThemeValidations();
        $scope.changeStatus();
    });

    $game.on('game_end', resp => {
        finishGame();
        setWinners(resp.winners);
        refreshGamePontuation();
    });

    $game.on('game_answers_time_elapsed', resp => {
        $scope.currentRoundTime = resp;
    });

    $game.on('validation_for_theme_started', resp => {
        startValidationTimeForTheme(resp);
    });    

    $game.on('validation_time_elapsed', resp => {
        refreshCurrentValidationTime(resp);
    });
    
}]);