angular.module('WeStop').controller('createGameController', ['$game', '$scope', '$location', '$rootScope', function ($game, $scope, $location, $rootScope) {

    $scope.game = {
        userName: $rootScope.user,
        gameOptions: {
            themes: [
                'Nome',
                'CEP',
                'Cor',
                'FDS',
                'Carro'
            ],
            availableLetters: [
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'I',
                'L',
                'M',
                'N',
                'O'
            ],
            rounds: 5,
            numberOfPlayers: 4
        }
    };

    $scope.confirm = function() {

        $game.invoke('games.create', $scope.game);

        $game.on('game.created', resp => {
            if (resp && resp.ok) {
                $location.path('/game/' + resp.game.id);
            }
        });
    };

}]);