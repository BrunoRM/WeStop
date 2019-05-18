angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', '$rootScope', '$user', function ($routeParams, $scope, $game, $rootScope, $user) {

    console.log($rootScope.user)
    $scope.allPlayersReady = false;
    $scope.gameStarted = false;
    $scope.stopCalled = false;
    $scope.playersAnswers = [];
    $scope.roundFinished = false;
    $scope.scoreboard = [];
    $scope.endGame = false;
    $scope.finalScoreboard = [];

    $game.invoke("game.join", { 
        gameId: $routeParams.id, 
        userId: $rootScope.user.id
    });

    $game.on("game.player.joined", (data) => {
        $scope.players = data.game.players;
        $scope.player = data.player;
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
        console.log(data)
        let player = $scope.players.find((player) => {
            return player.userName == data.player.userName;
        });

        if (!player)
            $scope.players.push(data.player);

        checkAllPlayersReady();
    });

    function checkAllPlayersReady() {
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

    $game.on('game.roundFinished', resp => {
        $scope.scoreboard = resp.scoreboard;
        $scope.roundFinished = true;
        $scope.allPlayersReady = false;
        $scope.gameStarted = false;
        $scope.stopCalled = false;
        $scope.playersAnswers = [];
        $scope.changeStatus();
    });

    $game.on('game.end', resp => {
        $scope.endGame = true;
        $scope.finalScoreboard = resp.finalScoreboard;
    });
    
}]);