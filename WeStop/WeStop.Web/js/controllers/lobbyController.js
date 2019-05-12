angular.module('WeStop').controller('lobbyController', ['$scope', '$location', '$game', '$rootScope', '$user', function ($scope, $location, $game, $rootScope, $user) {

    $scope.hasUser = $rootScope.user !== null;
    $scope.newUser = '';

    $scope.createUser = () => {
        if ($scope.newUser) {
            $user.create($scope.newUser);
            $scope.hasUser = true;
        }
    };

    $game.connect(function() {
        $game.invoke('games.get');
        $game.on('games.get.response', data => {
            $scope.gameRooms = data.games;
        });
    });

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

    $scope.newGame = () =>
        $location.path('/game/create');

}]);