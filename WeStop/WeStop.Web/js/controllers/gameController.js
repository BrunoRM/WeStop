angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', '$rootScope', '$user', function ($routeParams, $scope, $game, $rootScope, $user) {

    $scope.allPlayersReady = false;
    $scope.gameStarted = false;
    $scope.stopCalled = false;
    $scope.playersAnswers = [];
    $scope.roundFinished = false;
    $scope.scoreboard = [];
    $scope.endGame = false;
    $scope.finalScoreboard = [];
    $scope.pontuation = 0;

    $game.invoke("game.join", { 
        gameId: $routeParams.id, 
        userId: $rootScope.user.id
    });

    $game.on("game.player.joined", (data) => {
        $scope.gameName = data.game.name;
        $scope.pontuation = data.player.earnedPoints;
        $scope.currentRound = data.game.currentRound;
        $scope.numberOfRounds = data.game.rounds;
        $scope.players = data.game.players;
        $scope.player = data.player;
        $scope.scoreboard = data.game.scoreboard;
        checkAllPlayersReady();
    });

    $scope.startGame = () => {
        $scope.changeStatus();
        $game.invoke('game.startRound', { 
            gameRoomId: $routeParams.id, 
            userId: $rootScope.user.id
        });
    };

    $game.on('game.roundStarted', data => {
        $scope.gameConfig = data.gameRoomConfig;
        $scope.gameStarted = true;
        $scope.answers = [];

        for (let i = 0; i < $scope.gameConfig.themes.length; i++) {
            $scope.answers.push({
                theme: $scope.gameConfig.themes[i],
                answer: ''
            });
        }

        $scope.roundFinished = false;
    });

    $game.on('game.players.joined', data => {
        let player = $scope.players.find((player) => {
            return player.userName == data.player.userName;
        });

        if (!player)
            $scope.players.push(data.player);

        checkAllPlayersReady();
    });

    function checkAllPlayersReady() {
        
        if ($scope.players.length === 1) return false;

        $scope.allPlayersReady = !$scope.players.some(player => {
            return player.userName !== $scope.player.userName && !player.isReady;
        });
    }

    $scope.changeStatus = () => {
        
        $game.invoke('player.changeStatus', {
            gameId: $routeParams.id,
            userId: $rootScope.user.id,
            isReady: !$scope.player.isReady
        });
        
        var player = $scope.players.find((player) => {
            return player.userName == $scope.player.userName;
        });

        $scope.player.isReady = player.isReady = !$scope.player.isReady;        
    };
    
    $game.on('player.statusChanged', resp => {
        let player = $scope.players.find((player) => {
            return player.userName === resp.player.userName;
        });

        player.isReady = resp.player.isReady;
        checkAllPlayersReady();
    });

    $scope.stop = () => {
        
        $game.invoke('players.stop', {
            gameId: $routeParams.id,
            userId: $rootScope.user.id
        });

    };

    $game.on('players.stopCalled', (resp) => {

        $scope.stopCalled = true;

        $game.invoke('player.sendAnswers', {
            gameId: $routeParams.id,
            userId: $rootScope.user.id,
            answers: $scope.answers
        });

    });
    
    $scope.themeAnswersValidations = [];
    $game.on('player.answersSended', (resp) => {

        let answers = resp.answers;

        for (let i = 0; i < answers.length; i++) {
            
            let answer = answers[i];
            
            let themeValidation = $scope.themeAnswersValidations.find((validation) => validation.theme === answer.theme);

            if (!themeValidation) {
                let obj = {
                    theme: answer.theme,
                    answersValidations: [{
                            answer: answer.answer,
                            valid: true
                        }
                    ]
                }

                $scope.themeAnswersValidations.push(obj);
            } else {

                // Se a resposta já existir, não mostrar novamente
                if (themeValidation.answersValidations.find(av => av.answer === answer.answer)) return;

                themeValidation.answersValidations.push({
                    answer: answer.answer,
                    valid: true
                });
            }
        }
    });

    $scope.validate = (answersValidations) => {
        
        let obj = {
            gameId: $routeParams.id,
            userId: $rootScope.user.id,
            validation: answersValidations
        }
        
        $game.invoke('player.sendAnswersValidations', obj);        
    };

    $game.on('player.themeValidationsReceived', data => {
        $scope.themeAnswersValidations = $scope.themeAnswersValidations.filter((themeValidation) => {
            if (themeValidation.theme !== data.theme)
                return themeValidation;
        });
    });

    function getPlayerRoundPontuation(scoreboard) {
        return scoreboard.find(playerScore => {
            return playerScore.playerId === $rootScope.user.id;
        });
    };
    
    $game.on('game.roundFinished', resp => {
        $scope.scoreboard = resp.scoreboard;
        $scope.roundFinished = true;
        $scope.allPlayersReady = false;
        $scope.gameStarted = false;
        $scope.stopCalled = false;
        $scope.playersAnswers = [];
        $scope.changeStatus();
        $scope.currentRound++;
        $scope.gameConfig.currentRound.sortedLetter = '';

        let playerRoundPontuation = getPlayerRoundPontuation(resp.scoreboard);

        $scope.pontuation = playerRoundPontuation.gamePontuation;
    });

    $game.on('game.end', resp => {
        $scope.endGame = true;
        $scope.winners = resp.winners;

        let playerRoundPontuation = getPlayerRoundPontuation(resp.scoreboard);

        $scope.pontuation = playerRoundPontuation.gamePontuation;
        $scope.scoreboard = resp.scoreboard;
    });
    
}]);