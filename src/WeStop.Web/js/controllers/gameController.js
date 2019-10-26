angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', '$rootScope', '$mdToast', '$location', function ($routeParams, $scope, $game, $rootScope, $mdToast, $location) {

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
        $game.invoke("join", $routeParams.id, '', $rootScope.user);
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

    $game.on("im_joined_game", (resp) => {
        setGame(resp.game);
        setPlayer(resp.player);
        addRoundNumber();
        refreshGamescoreboard(resp.lastRoundScoreboard);
        checkAllPlayersReady();   
    });

    $game.on('im_reconected_game', (resp) => {
        setGame(resp.game);
        setPlayer(resp.player);

        switch (resp.game.state) {
            case 'InProgress':
                setSortedLetter(resp.round.sortedLetter);
                startRound();
                break;
            case 'Validations':
                setSortedLetter(resp.round.sortedLetter);
                if (resp.validated) {
                    cleanThemeValidations();
                } else {
                    setCurrentValidation(resp);
                    startValidation();
                }
                break;
            case 'Finished':
                setWinners(resp.winners);
                refreshGamescoreboard(resp.lastRoundScoreboard);
                finishGame();
                updateGamePontuation();
                break;
        }
    });

    $scope.startRound = () => {
        $scope.changeStatus();
        $game.invoke('start_round', $routeParams.id);
    };    

    $game.on('round_started', data => {
        setSortedLetter(data.sortedLetter);
        startRound();
    });

    $game.on('player_joined_game', data => {
        let player = $scope.game.players.find((player) => {
            return player.id === data.player.id;
        });

        if (!player) {
            $scope.game.players.push(data.player);
            $mdToast.show(
                $mdToast.simple()
                    .textContent(data.player.userName + ' entrou na partida')
                    .position('bottom left')
                    .hideDelay(1500)
            );
        }

        checkAllPlayersReady();
    });

    $scope.changeStatus = () => {

        $game.invoke('player_change_status', 
            $routeParams.id,
            $rootScope.user.id,
            !$scope.player.isReady
        );
    };

    $game.on('im_change_status', (resp) => {
        let player = getPlayer($scope.player.id);
        $scope.player.isReady = player.isReady = resp.isReady;
    });
    
    $game.on('player_changed_status', resp => {
        let player = getPlayer(resp.id);
        player.isReady = resp.isReady;
        checkAllPlayersReady();
    });

    $scope.stop = () => {
        $game.invoke('stop_round', $routeParams.id, $rootScope.user.id);
    };
    
    $game.on('round_stoped', (resp) => {
        
        if (resp.reason === 'player_call_stop') {
            
            if (resp.playerId !== $scope.player.id) {
                let playerThatCallStop = getPlayer(resp.playerId);
                $mdToast.show(
                    $mdToast.simple()
                        .textContent(playerThatCallStop.userName + ' chamou STOP!')
                        .position('bottom left')
                        .hideDelay(3500)
                );                
            }
        }
        
        stop();
        $game.invoke('send_answers', {
            playerId: $rootScope.user.id,
            gameId: $routeParams.id,
            roundNumber: $scope.game.currentRoundNumber,
            answers: $scope.answers
        });

    });
    
    $game.on('im_send_validations', data => {
        cleanThemeValidations();
    });    

    $game.on('round_finished', resp => {
        init();
        refreshGamescoreboard(resp.scoreboard);
        updateGamePontuation();
        addRoundNumber();
        cleanThemeValidations();
        $scope.changeStatus();
    });

    $game.on('game_finished', resp => {
        init();
        finishGame();
        refreshGamescoreboard(resp.lastRoundScoreboard);
        setWinners(resp.winners);
        updateGamePontuation();
    });    

    function calculateTimePercentage(limitTime, time) {
        return (time * 100) / limitTime;
    }

    $game.on('round_time_elapsed', resp => {
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
    
    $game.on('validation_started', resp => {
        setCurrentValidation(resp);
        startValidation();
    });    

    $game.on('validation_time_elapsed', resp => {
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

    $game.on('validation_time_over', () => {
        $scope.validationTimeOver = true;
        sendValidationsAfterTimeOver();
    });
    
    function sendValidationsAfterTimeOver() {
        let data = buildValidationData();
        $game.invoke('send_validations_after_time_over', data);
    }

    $scope.finishValidation = function () {
        let data = buildValidationData();
        $game.invoke('send_validations', data);
    };

    $game.on('player_left', (data) => {
        let player = getPlayer(data);
        if (player) {
            let userName = player.userName;
            removePlayer(player);
            if (data !== $scope.player.id) {
                $mdToast.show(
                    $mdToast.simple()
                        .textContent(userName + ' deixou a partida')
                        .position('bottom left')
                        .hideDelay(1500)
                );
            }
        }
    });

    function removePlayer(player) {
        let indexOfPlayer = getIndexOfPlayer(player);
        if (indexOfPlayer > -1) {
            $scope.game.players.splice(indexOfPlayer, 1);
        }
    }

    $game.on('new_admin_setted', (data) => {
        let player = getPlayer(data);
        setAdminTo(player);
    });

    function setAdminTo(player) {
        let indexOfPlayer = getIndexOfPlayer(player);
        if (indexOfPlayer > -1) {
            $scope.game.players[indexOfPlayer].isAdmin = true;
            if (player.id === $scope.player.id) {
                $scope.player.isAdmin = true;
                $mdToast.show(
                    $mdToast.simple()
                        .textContent('Você é o administrador da partida')
                        .position('bottom left')
                        .hideDelay(2500)
                );
            }
        }
    }

    function getIndexOfPlayer(player) {
        return $scope.game.players.indexOf(player);
    }

    $scope.leave = () => {
        $rootScope.isLoading = true;
        $game.invoke('leave', $routeParams.id, $rootScope.user.id).then(function () {
            $game.leave();
            $rootScope.isLoading = false;
            $location.path('/lobby');            
        });
    };

    init();
    joinGame();
    
}]);