angular.module('WeStop').controller('lobbyController', ['gameRooms', '$scope', '$location', function (gameRooms, $scope, $location) {

    $scope.gameRooms = gameRooms;

    $scope.gameRoomDetails = null;
    $scope.showGameRommDetails = false;

    $scope.details = function(gameRoom) {

        $scope.gameRoomDetails = gameRoom;
        $scope.showGameRommDetails = true;

    };

    $scope.backToGameRooms = function () {
        $scope.gameRoomDetails = null;
        $scope.showGameRommDetails = false;
    };

    $scope.joinGame = () => 
        $location.path('/game/' + $scope.gameRoomDetails.id);

}]);