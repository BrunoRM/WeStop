angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', '$rootScope', '$mdToast', function ($routeParams, $scope, $game, $rootScope, $mdToast) {

    function init() {
        $scope.allPlayersReady = false;
        $scope.roundStarted = false;
        $scope.roundStopped = false;
        $scope.gameFinished = false;
        $scope.validationStarted = false;

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

        $scope.currentValidationTime = 0;
    }

    function finishGame() {
        $scope.allPlayersReady = false;
        $scope.roundStarted = false;
        $scope.roundStopped = false;
        $scope.gameFinished = true;
        $scope.validationStarted = false;
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

        if (!player)
            $scope.game.players.push(data.player);

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
        $game.invoke('stop_round', $routeParams.id, $scope.game.currentRoundNumber, $rootScope.user.id);
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
        $scope.currentValidationTimePercentage = calculateTimePercentage(15, resp.currentTime);
        refreshCurrentValidationTime(resp.currentTime);
    });

    $game.on('validation_time_over', () => $scope.finishValidation());

    $scope.finishValidation = function () {
        
        let data = {
            gameId: $routeParams.id,
            playerId: $rootScope.user.id,
            roundNumber: $scope.game.currentRoundNumber,
            theme: $scope.currentValidation.theme,
            validations: $scope.currentValidation.validations
        };

        $game.invoke('send_validations', data);
    };

    init();
    joinGame();
    
}]);