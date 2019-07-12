angular.module('WeStop').controller('createGameController', ['$game', '$scope', '$location', '$rootScope', '$http', 'API_SETTINGS', function ($game, $scope, $location, $rootScope, $http, API_SETTINGS) {

    $scope.themes = [];
    $http.get(API_SETTINGS.uri + '/themes.list').then((resp) => {

        resp.data.themes.forEach(theme => {
            $scope.themes.push({
                name: theme.name,
                enabled: true
            })
        }); 
    });

    $scope.themeToAdd = {};

    $scope.availableLetters = [
        { letter: 'A', enabled: true },
        { letter: 'B', enabled: true },
        { letter: 'C', enabled: true },
        { letter: 'D', enabled: true },
        { letter: 'E', enabled: true },
        { letter: 'F', enabled: true },
        { letter: 'G', enabled: true },
        { letter: 'H', enabled: true },
        { letter: 'I', enabled: true },
        { letter: 'J', enabled: true },
        { letter: 'K', enabled: false },
        { letter: 'L', enabled: true },
        { letter: 'M', enabled: true },
        { letter: 'N', enabled: true },
        { letter: 'O', enabled: true },
        { letter: 'P', enabled: true },
        { letter: 'Q', enabled: true },
        { letter: 'R', enabled: true },
        { letter: 'S', enabled: true },
        { letter: 'T', enabled: true },
        { letter: 'U', enabled: true },
        { letter: 'V', enabled: true },
        { letter: 'W', enabled: false },
        { letter: 'X', enabled: false },
        { letter: 'Y', enabled: false },
        { letter: 'Z', enabled: false }
    ];

    $scope.game = {
        userId: $rootScope.user.id,
        gameOptions: {
            themes: [],
            availableLetters: [],
            rounds: "4",
            numberOfPlayers: "6",
            time: "30"
        }
    };

    $scope.changeStatus = (letter) => {
        letter.enabled = !letter.enabled;
    };

    $scope.confirmLetters = () => {
        
        $scope.game.gameOptions.availableLetters = [];
        $scope.availableLetters.forEach((availableLetter) => {
            if (availableLetter.enabled)
                $scope.game.gameOptions.availableLetters.push(availableLetter.letter);
        });

        $scope.nextStep();
    };

    $scope.confirmThemes = () => {
        $scope.game.gameOptions.themes = [];
        $scope.themes.forEach((theme) => {
            if (theme.enabled)
                $scope.game.gameOptions.themes.push(theme.name);
        });
        
        $scope.confirm();
    };

    $scope.currentStep = 1;
    $scope.nextStep = () => {
        $scope.currentStep++;
    };

    $scope.previousStep = () => {
        $scope.currentStep--;
    };
    
    $scope.addTheme = () => {

        if ($scope.themeToAdd && $scope.themeToAdd.name !== '') {
            $scope.themes.push({ 
                name: $scope.themeToAdd.name, 
                enabled: true 
            });
            $scope.themeToAdd = {};
        }
    };

    $scope.confirm = function() {

        $game.invoke('game_create', $scope.game);

        $game.on('game_created', resp => {
            if (resp && resp.ok) {
                $location.path('/game/' + resp.game.id);
            }
        });
    };

    $scope.goToLobby = () => {
        $location.path('/lobby');
    };

}]);