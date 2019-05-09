angular.module('WeStop').controller('createGameController', ['$game', '$scope', '$location', function ($game, $scope, $location) {

    $scope.game = {
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

        $game.connect(function () {
            $game.invoke('createGame', $scope.game);
            $game.on('gameCreated', resp => {
                if (resp && resp.ok) {
                    $location.path('/game/' + resp.game.id);
                }
            });
        }, (error) => {
            console.log(error);
        });
    };

}]);