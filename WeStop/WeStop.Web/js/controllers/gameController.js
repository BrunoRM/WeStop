angular.module('WeStop').controller('gameController', ['$routeParams', '$scope', function ($routeParams, $scope) {

    $scope.userNameValidated = false;
    $scope.player = {
        userName: ''
    }

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/gameroom")
        .build();

    connection.start().then((resp) => {
    }, (error) => {
        console.log(error);
    });
    
    connection.on("error", data => {
        console.log(data);
    });
    
    $scope.confirm = () => {
        
        if ($scope.player.userName === '')
            return;
        
        connection.invoke("join", { gameRoomId: $routeParams.id, playerId: $scope.player.userName });

        connection.on("connected", data => {
            console.log(data);
            $scope.$apply(() => {
                $scope.userNameValidated = true;
                $scope.player.isAdmin = data.is_admin;
            });
        });
    };

    $scope.startGame = () => {

        connection.invoke('startGame', { gameRoomId: $routeParams.id, playerId: $scope.player.userName });

        connection.on('gameStarted', data => {
            console.log(data);
        });
    };

    
}]);