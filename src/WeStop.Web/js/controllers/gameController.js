angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$hub', '$rootScope', '$toast', '$location', '$mdDialog', function ($routeParams, $scope, $hub, $rootScope, $toast, $location, $mdDialog) {

    let hubConnection = $hub.createConnection('/game');    

    function init() {
        $scope.allPlayersReady = false;
        $scope.roundStarted = false;
        $scope.roundStopped = false;
        $scope.gameFinished = false;
        $scope.validationStarted = false;
        $scope.validationTimeOver = false;

        $scope.sortedLetter = '?';
        $scope.answersTime = 0;
        $scope.currentAnswersTimePercentage = 0;
        $scope.currentValidationTime = 0;
        $scope.currentValidationTimePercentage = 0;
        $scope.themeValidations = null;
    }

    function joinGame() {
        $hub.invoke(hubConnection, "join", $routeParams.id, $rootScope.user);
    }

    function setGame(game) {
        $scope.game = game;
        $scope.game.players.sort(function (p, n) {
            return p.id == $rootScope.user.id ? -1 : n.id == $rootScope.user.id ? 1 : 0;
        });
    }
    
    function setPlayer(player) {
        $scope.player = player;
    }
    
    function startRound() {        
        $scope.allPlayersReady = true;
        $scope.roundStarted = true;
        $scope.roundStopped = false;
        $scope.gameFinished = false;
        $scope.validationStarted = false;
        $scope.validationTimeOver = false;

        $scope.answers = [];
        for (let i = 0; i < $scope.game.themes.length; i++) {
            $scope.answers.push({
                theme: $scope.game.themes[i],
                answer: ''
            });
        }
    }

    function startValidation() {
        $scope.allPlayersReady = true;
        $scope.roundStarted = true;
        $scope.roundStopped = true;
        $scope.gameFinished = false;
        $scope.validationStarted = true;
        $scope.validationTimeOver = false;

        $scope.currentValidationTime = 0;
    }

    function finishGame() {
        $scope.allPlayersReady = false;
        $scope.roundStarted = false;
        $scope.roundStopped = false;
        $scope.gameFinished = true;
        $scope.validationStarted = false;
        $scope.validationTimeOver = true;
    }

    function getPlayerGamePontuation(playerId) {
        let playerScore = $scope.scoreboard.find(playerScore => {
            return playerScore.playerId === playerId;
        });

        if (playerScore)
            return playerScore.gamePontuation;
        else
            return 0;
    }

    function checkAllPlayersReady() {
        if ($scope.game.players.length === 1) {
            $scope.allPlayersReady = false;
            return;
        }

        $scope.allPlayersReady = !$scope.game.players.some(player => {
            return player.id !== $scope.player.id && !player.isReady;
        });
    }

    function refreshCurrentValidationTime(newValue) {
        $scope.currentValidationTime = newValue;
    }

    function cleanThemeValidations() {
        $scope.currentValidation = null;
    }

    function stop() {
        $scope.roundStopped = true;
        $scope.roundTime = 0;
    }

    function updateGamePontuation() {
        for (let i = 0; i < $scope.game.players.length; i++) {
            let player = $scope.game.players[i];
            player.totalPontuation = getPlayerGamePontuation(player.id);
        }
    }

    function setWinners(winners) {
        $scope.winners = winners;
    }

    function setSortedLetter(letter) {
        $scope.sortedLetter = letter;
    }

    function refreshGamescoreboard(scoreboard) {
        $scope.scoreboard = scoreboard;
    }

    function getPlayer(id) {
        return $scope.game.players.find((player) => {
            return player.id === id;
        });
    }

    function refreshCurrentAnswersTime(newValue) {
        $scope.currentAnswersTime = newValue;
    }

    function addRoundNumber() {
        $scope.game.currentRoundNumber += 1;
    }

    $hub.on(hubConnection, "im_joined_game", (resp) => {
        setGame(resp.game);
        setPlayer(resp.player);        
        refreshGamescoreboard(resp.lastRoundScoreboard);
        checkAllPlayersReady();   

        if (resp.game.state === 'InProgress') {
            setSortedLetter(resp.round.sortedLetter);
            startRound();
        } else {
            addRoundNumber();
        }
    });

    $scope.startRound = () => {
        $scope.changeStatus();
        $hub.invoke(hubConnection, 'start_round', $routeParams.id);
    };    

    $hub.on(hubConnection, 'round_started', data => {
        setSortedLetter(data.sortedLetter);
        let indexOfLetter = $scope.game.availableLetters.indexOf(data.sortedLetter);
        $scope.game.availableLetters.splice(indexOfLetter, 1);
        $scope.game.sortedLetters.push(data.sortedLetter);
        startRound();
    });

    $hub.on(hubConnection, 'player_joined_game', data => {
        let player = $scope.game.players.find((player) => {
            return player.id === data.player.id;
        });

        if (!player) {
            $scope.game.players.push(data.player);
            $toast.show(data.player.userName + ' entrou na partida');
        }

        checkAllPlayersReady();
    });

    $hub.on(hubConnection, 'game_join_error', (reason) => {
        switch (reason) {
            case 'GAME_NOT_FOUND':
                $toast.show('Essa partida não existe mais');
                break;
            case 'GAME_FULL':
                $toast.show('Essa partida está cheia');
                break;
            case 'PLAYER_NOT_AUTHORIZED':
                $toast.show('Você não está autorizado para entrar nessa partida');
                break;

            }
        $scope.goToLobby();
    });

    $scope.changeStatus = () => {
        $hub.invoke(hubConnection, 'player_change_status', 
            $routeParams.id,
            $rootScope.user.id,
            !$scope.player.isReady
        );
    };

    $hub.on(hubConnection, 'im_change_status', (resp) => {
        let player = getPlayer($scope.player.id);
        $scope.player.isReady = player.isReady = resp.isReady;
    });
    
    $hub.on(hubConnection, 'player_changed_status', resp => {
        let player = getPlayer(resp.id);
        player.isReady = resp.isReady;
        checkAllPlayersReady();
    });

    $scope.stop = () => {
        $hub.invoke(hubConnection, 'stop_round', $routeParams.id, $rootScope.user.id);
    };
    
    $hub.on(hubConnection, 'round_stoped', (resp) => {
        
        if (resp.reason === 'player_call_stop') {
            
            if (resp.playerId !== $scope.player.id) {
                let playerThatCallStop = getPlayer(resp.playerId);
                $toast.show(playerThatCallStop.userName + ' chamou STOP!');
            }
        }
        
        stop();
        $hub.invoke(hubConnection, 'send_answers', {
            playerId: $rootScope.user.id,
            gameId: $routeParams.id,
            roundNumber: $scope.game.currentRoundNumber,
            answers: $scope.answers
        });

    });
    
    $hub.on(hubConnection, 'im_send_validations', data => {
        cleanThemeValidations();
    });    

    $hub.on(hubConnection, 'round_finished', resp => {
        init();
        refreshGamescoreboard(resp.scoreboard);
        updateGamePontuation();
        addRoundNumber();
        cleanThemeValidations();
        $scope.changeStatus();
    });

    $hub.on(hubConnection, 'game_finished', resp => {
        init();
        finishGame();
        refreshGamescoreboard(resp.lastRoundScoreboard);
        setWinners(resp.winners);
        updateGamePontuation();
    });

    $scope.roundPontuations = [];
    $hub.on(hubConnection, 'receive_my_pontuations_in_round', resp => {
        $scope.roundPontuations.push(resp);
    });

    function calculateTimePercentage(limitTime, time) {
        return (time * 100) / limitTime;
    }

    $hub.on(hubConnection, 'round_time_elapsed', resp => {
        $scope.currentAnswersTimePercentage = calculateTimePercentage($scope.game.time, resp);
        refreshCurrentAnswersTime(resp);        
    });

    function setCurrentValidation(validationData) {
        if (validationData.validations && validationData.validations.length !== 0) {
            $scope.currentValidationTimePercentage = 0;
            $scope.currentValidation = {
                number: validationData.validationsNumber,
                total: validationData.totalValidations,
                theme: validationData.theme,
                validations: validationData.validations
            };
        }
    }
    
    $hub.on(hubConnection, 'validation_started', resp => {
        setCurrentValidation(resp);
        startValidation();
    });    

    $hub.on(hubConnection, 'validation_time_elapsed', resp => {
        $scope.currentValidationTimePercentage = calculateTimePercentage(10, resp.currentTime);
        refreshCurrentValidationTime(resp.currentTime);
    });

    function buildValidationData() {
        return {
            gameId: $routeParams.id,
            playerId: $rootScope.user.id,
            roundNumber: $scope.game.currentRoundNumber,
            theme: $scope.currentValidation.theme,
            validations: $scope.currentValidation.validations
        };
    }

    $hub.on(hubConnection, 'validation_time_over', () => {
        $scope.validationTimeOver = true;
        sendValidationsAfterTimeOver();
    });
    
    function sendValidationsAfterTimeOver() {
        let data = buildValidationData();
        $hub.invoke(hubConnection, 'send_validations_after_time_over', data);
    }

    $scope.finishValidation = function () {
        let data = buildValidationData();
        $hub.invoke(hubConnection, 'send_validations', data);
    };

    $hub.on(hubConnection, 'player_left', (data) => {
        let player = getPlayer(data);
        if (player) {
            removePlayer(player);
            if (player.id !== $scope.player.id) {
                $toast.show(player.userName + ' deixou a partida');
                $scope.game.numberOfPlayers--;
            }
        }
    });

    function removePlayer(player) {
        let indexOfPlayer = getIndexOfPlayer(player);
        if (indexOfPlayer > -1) {
            $scope.game.players.splice(indexOfPlayer, 1);
        }
    }

    $hub.on(hubConnection, 'new_admin_setted', (data) => {
        let player = getPlayer(data);
        setAdminTo(player);
    });

    function setAdminTo(player) {
        let indexOfPlayer = getIndexOfPlayer(player);
        if (indexOfPlayer > -1) {
            $scope.game.players[indexOfPlayer].isAdmin = true;
            if (player.id === $scope.player.id) {
                $scope.player.isAdmin = true;
                $toast.show('Você é o administrador da partida');
            }
        }
    }

    function getIndexOfPlayer(player) {
        return $scope.game.players.indexOf(player);
    }

    $scope.leave = () => {
        let confirm = $mdDialog.confirm()
            .title('Confirmação')
            .textContent('Tem certeza que quer sair da partida?')
            .ok('Sim')
            .cancel('Não');

        $mdDialog.show(confirm).then(function () {
            if ($scope.gameFinished) {
                $scope.goToLobby();
            } else {
                $rootScope.isLoading = true;
                $hub.invoke(hubConnection, 'leave', $routeParams.id, $rootScope.user.id).then(function () {
                    $rootScope.isLoading = false;
                    $scope.goToLobby();
                }, () => $rootScope.isLoading = false);
            }
        }, function () {
        });
    };

    $hub.on(hubConnection, 'im_left', () => $scope.goToLobby());

    $scope.goToLobby = () => {
        $hub.stop(hubConnection);
        $location.path('/lobby');
    };

    init();
    $hub.connect(hubConnection, () => {
        joinGame();
    });
    
}]);