angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', '$rootScope', '$user', function ($routeParams, $scope, $game, $rootScope, $user) {

    $scope.allPlayersReady = false;
    $scope.gameStarted = false;
    $scope.player = {
        userName: $rootScope.user
    }
    $scope.stopCalled = false;
    $scope.playersAnswers = [];
    $scope.roundFinished = false;

    $game.invoke("game.join", { 
        gameRoomId: $routeParams.id, 
        userName: $scope.player.userName 
    });

    $game.on("game.player.joined", (data) => {
        $scope.players = data.game.players;
        $scope.player = data.player;
        checkAllPlayersReady();
    });

    $scope.startGame = () => {
        $game.invoke('game.startRound', { 
            gameRoomId: $routeParams.id, 
            userName: $scope.player.userName 
        });
    };

    $game.on('game.roundStarted', data => {
        $scope.gameConfig = data.gameRoomConfig;
        $scope.gameStarted = true;
        $scope.answers = { }

        for (let i = 0; i < $scope.gameConfig.themes.length; i++) {
            $scope.answers[$scope.gameConfig.themes[i]] = '';
        }
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
        $scope.allPlayersReady = !$scope.players.some(player => {
            return player.userName !== $scope.player.userName && !player.isReady;
        });
    }

    $scope.changeStatus = () => {
        
        $game.invoke('player.changeStatus', {
            gameId: $routeParams.id,
            userName: $scope.player.userName,
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
            userName: $scope.player.userName
        });

    };

    $game.on('players.stopCalled', (resp) => {

        $scope.stopCalled = true;
        if (resp.userName !== $scope.player.userName) {
            alert(resp.userName + ' chamou STOP');
        }

        $game.invoke('player.sendAnswers', {
            gameId: $routeParams.id,
            userName: $scope.player.userName,
            answers: $scope.answers
        });

        $scope.roundFinished = true;

    });
    
    $scope.answersValidations = [];
    $game.on('player.answers', (resp) => {

        let answers = resp.answers;

        for (let key in answers) {

            let themeValidation = $scope.answersValidations.find((validation) => validation.theme === key);

            if (!themeValidation) {
                let obj = {
                    theme: key,
                    validations: {}
                }

                obj.validations[answers[key]] = true;
                $scope.answersValidations.push(obj);
            } else {
                if (!themeValidation.validations[answers[key]])
                    themeValidation.validations[answers[key]] = true;
            }
        }
    });

    $scope.validate = (answersValidations) => {
        console.log(answersValidations);
        
        let obj = {
            gameId: $routeParams.id,
            userName: $user.get(),
            validation: answersValidations
        }
        
        $game.invoke('player.sendAnswersValidations', obj);        
    };

    $game.on('player.themeValidationsReceived', data => {
        console.log(data);
    });
    
}]);