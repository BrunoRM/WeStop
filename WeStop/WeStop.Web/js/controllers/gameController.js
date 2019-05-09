angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', '$game', function ($routeParams, $scope, $game) {

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

    
}]);