angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', function ($routeParams, $scope, $game) {

    $scope.allPlayersReady = false;
    $scope.userNameValidated = false;
    $scope.gameStarted = false;
    $scope.player = {
        userName: ''
    }
    
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

    $game.on('gameStarted', data => {
        console.log(data);
        $scope.gameConfig = data.gameRoomConfig;
        $scope.gameStarted = true;
    });

    $game.on('playerJoinedToGame', data => {
        $scope.players.push(data.player);
    });

    function checkAllPlayersReady() {

        // for (let i = 0; i < $scope.players.length; i++) {

        //     let player = $scope.players[i];

        //     if (player.userName == $scope.player.userName) continue;
        //     else if (player.userName !== $scope.player.userName && !player.isReady) {
        //         $scope.allPlayersReady = false;
        //         break;
        //     } else if (player.isReady) continue;

        //     $scope.allPlayersReady = true;            
        // }

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
    
}]);