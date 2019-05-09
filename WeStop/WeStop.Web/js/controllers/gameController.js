angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', function ($routeParams, $scope, $game) {

    $scope.allPlayersReady = false;
    $scope.userNameValidated = false;
    $scope.gameStarted = false;
    $scope.player = {
        userName: ''
    }
    $scope.stopCalled = false;
    $scope.playersAnswers = [];
    $scope.roundFinished = false;
    
    $scope.confirm = () => {
        
        if ($scope.player.userName === '')
            return;

        $game.invoke("join", { gameRoomId: $routeParams.id, userName: $scope.player.userName });
    };

    $game.on("joinedToGame", (data) => {
        $scope.userNameValidated = true;
        $scope.players = data.game.players;
        $scope.player.isAdmin = data.is_admin;
    });

    $scope.startGame = () => {
        $game.invoke('startGame', { gameRoomId: $routeParams.id, userName: $scope.player.userName });
    };

    $game.on('roundStarted', data => {
        $scope.gameConfig = data.gameRoomConfig;
        $scope.gameStarted = true;
        $scope.answers = { }

        for (let i = 0; i < $scope.gameConfig.themes.length; i++) {
            $scope.answers[$scope.gameConfig.themes[i]] = '';
        }
    });

    $game.on('playerJoinedToGame', data => {
        $scope.players.push(data.player);
    });

    function checkAllPlayersReady() {
        $scope.allPlayersReady = !$scope.players.some(player => {
            return player.userName !== $scope.player.userName && !player.isReady;
        });
    }

    $game.on('player.statusChanged', resp => {
        
        var player = $scope.players.find((player) => {
            return player.id == resp.player.id;
        });

        player.isReady = resp.player.isReady;
        checkAllPlayersReady();
    });

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

    $scope.stop = () => {

        $game.invoke('stop', {
            gameId: $routeParams.id,
            userName: $scope.player.userName
        });

    };

    $game.on('stopCalled', (resp) => {

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
    
    $game.on('player.answersReceived', (resp) => {
        $scope.playersAnswers.push({
            userName: resp.userName,
            answers: resp.answers
        });

    });
    
}]);