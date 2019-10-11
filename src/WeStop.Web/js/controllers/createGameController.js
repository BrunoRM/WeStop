angular.module('WeStop').controller('createGameController', ['$game', '$scope', '$location', '$rootScope', '$http', 'API_SETTINGS', function ($game, $scope, $location, $rootScope, $http, API_SETTINGS) {

    $scope.selectedThemes = [];
    let availableThemes = [];

    function drawnThemes(count) {
        $scope.selectedThemes = [];
        for (let i = 0; i < count; i++) {
            let notDrawnThemes = availableThemes.filter((theme) => !$scope.selectedThemes.includes(theme));
            let randomNumber = Math.floor(Math.random() * notDrawnThemes.length);
            let raffledTheme = notDrawnThemes[randomNumber];
            let indexOfRaffledTheme = availableThemes.indexOf(raffledTheme);
            $scope.selectedThemes.push(notDrawnThemes[indexOfRaffledTheme]);
        }
    };
    
    $http.get(API_SETTINGS.uri + '/themes.list').then((resp) => {
        resp.data.themes.forEach(theme => {
            availableThemes.push(theme.name);
        });

        drawnThemes(6);
    });
    
    $scope.drawnAnotherThemes = function() {
        drawnThemes(6);
    };

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
        user: $rootScope.user,
        gameOptions: {
            availableThemes: [],
            availableLetters: [],
            rounds: "4",
            numberOfPlayers: "6",
            roundTime: "30"
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
        $scope.game.gameOptions.availableThemes = [];
        $scope.selectedThemes.forEach((theme) => {
            if (theme.enabled)
                $scope.game.gameOptions.availableThemes.push(theme.name);
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
            $scope.selectedThemes.push({ 
                name: $scope.themeToAdd.name, 
                enabled: true 
            });
            $scope.themeToAdd = {};
        }
    };

    $scope.confirm = function() {

        $http.post(API_SETTINGS.uri + '/games.create', $scope.game).then((resp) =>
        {
            $location.path('/game/' + resp.data.id);
        });
    };

    $scope.goToLobby = () => {
        $location.path('/lobby');
    };

}]);