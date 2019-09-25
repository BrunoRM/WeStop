angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', '$rootScope', function ($routeParams, $scope, $game, $rootScope) {

    function init() {
        $scope.allPlayersReady = false;
        $scope.roundStarted = false;
        $scope.roundStopped = false;
        $scope.gameFinished = false;
        $scope.validationsStarted = false;

        $scope.sortedLetter = '';
        $scope.currentAnswersTime = 0;
        $scope.currentValidationTime = 0;
        $scope.themeValidations = null;        
    };

    function joinGame() {
        $game.invoke("join", $routeParams.id, $rootScope.user.id);
    };

    function setGame(game) {
        $scope.game = game;
    };
    
    function setPlayer(player) {
        $scope.player = player;
    };
    
    function startRound() {
        
        $scope.allPlayersReady = true;
        $scope.roundStarted = true;
        $scope.roundStopped = false;
        $scope.gameFinished = false;
        $scope.validationsStarted = false;

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

    function getPlayerGamePontuation() {
        let playerScore = $scope.game.scoreBoard.find(playerScore => {
            return playerScore.playerId === $rootScope.user.id;
        });

        return playerScore.gamePontuation;
    };

    function checkAllPlayersReady() {

        if ($scope.game.players.length === 1) {
            $scope.allPlayersReady = false;
        }

        $scope.allPlayersReady = !$scope.game.players.some(player => {
            return player.id !== $scope.player.id && !player.isReady;
        });
    }

    function refreshCurrentValidationTime(newValue) {
        $scope.currentValidationTime = newValue;
    }

    function cleanThemeValidations() {
        $scope.themeValidations = null;
    }

    function stop() {
        $scope.roundStopped = true;
        $scope.roundTime = 0;
    }

    function updateGamePontuation() {
        $scope.player.earnedPoints = getPlayerGamePontuation();
    }

    function setWinners(winners) {
        $scope.winners = winners;
    }

    function setThemeValidation(themeValidation) {
        $scope.themeValidations = themeValidation;
    }

    function setSortedLetter(letter) {
        $scope.sortedLetter = letter;
    }

    function refreshGameScoreBoard(scoreBoard) {
        $scope.game.scoreBoard = scoreBoard;
    }

    function getPlayer(id) {
        return $scope.game.players.find((player) => {
            return player.id === id;
        });
    }

    function refreshCurrentAnswersTime(newValue) {
        $scope.currentAnswersTime = newValue;
    }

    function addGameCurrentRoundNumber() {
        $scope.game.currentRound += 1;
    }

    $game.on("im_joined_game", (data) => {
        setGame(data.game);
        setPlayer(data.player);
        checkAllPlayersReady();   
    });

    $game.on('im_reconected_game', (resp) => {

        setGame(resp.game);
        setPlayer(resp.player);
        startRound();

        switch (resp.game.state) {
            case "ThemesValidations":
                if (resp.validated) {
                    cleanThemeValidations();
                } else {
                    startValidation();
                    setThemeValidation(resp.themeValidations);
                }
                break;
            case "Finished":
                setWinners(resp.winners);
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
            return player.userName == data.player.userName;
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

        let player = getPlayer($scope.player.id);
        $scope.player.isReady = player.isReady = !$scope.player.isReady;
    };    
    
    $game.on('player_changed_status', resp => {
        let player = getPlayer(resp.player.id);
        player.isReady = resp.player.isReady;
        checkAllPlayersReady();
    });

    $scope.stop = () => {

        $game.invoke('stop_round', $routeParams.id, $rootScope.user.id);

    };

    $game.on('round_stoped', (resp) => {
        
        if (resp.reason === 'player_call_stop') {
            //alert(resp.userName + ' chamou STOP!');
        }
        
        stop();
        $game.invoke('send_answers', {
            playerId: $rootScope.user.id,
            gameId: $routeParams.id,
            roundNumber: $scope.game.currentRound,
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

    $game.on('game_round_finished', resp => {
        init();
        refreshGameScoreBoard(resp.scoreBoard);
        updateGamePontuation();
        cleanThemeValidations();
        $scope.changeStatus();
        addGameCurrentRoundNumber();
        cleanThemeValidations();
    });

    $game.on('game_end', resp => {
        finishGame();
        refreshGameScoreBoard(resp.scoreBoard);
        setWinners(resp.winners);
        updateGamePontuation();
        addGameCurrentRoundNumber();
    });    

    $game.on('round_time_elapsed', resp => {
        refreshCurrentAnswersTime(resp);        
    });

    $game.on('validation_started', resp => {
        console.log(resp);
        setThemeValidation(resp);
        startValidation();
    });    

    $game.on('validation_time_elapsed', resp => {
        refreshCurrentValidationTime(resp);
    });

    init();
    joinGame();
    
}]);