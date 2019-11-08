angular.module('WeStop').controller('createGameController', ['$scope', '$location', '$rootScope', '$http', 'API_SETTINGS', function ($scope, $location, $rootScope, $http, API_SETTINGS) {
    
    let availableThemes = [];

    function drawnThemes(count) {
        $scope.game.gameOptions.themes = [];
        for (let i = 0; i < count; i++) {
            let notDrawnThemes = availableThemes.filter((theme) => !$scope.game.gameOptions.themes.includes(theme));
            let randomNumber = Math.floor(Math.random() * notDrawnThemes.length);
            let raffledTheme = notDrawnThemes[randomNumber];
            $scope.game.gameOptions.themes.push(raffledTheme);
        }
    }
    
    $http.get(API_SETTINGS.uri + '/api/themes.list').then((resp) => {
        resp.data.themes.forEach(theme => {
            availableThemes.push(theme.name);
        });

        drawnThemes(6);
    });
    
    $scope.drawnAnotherThemes = function() {
        drawnThemes(6);
    };

    $scope.themeToAdd = {};

    $scope.letters = [
        { value: 'A', enabled: true },
        { value: 'B', enabled: true },
        { value: 'C', enabled: true },
        { value: 'D', enabled: true },
        { value: 'E', enabled: true },
        { value: 'F', enabled: true },
        { value: 'G', enabled: true },
        { value: 'H', enabled: false },
        { value: 'I', enabled: true },
        { value: 'J', enabled: true },
        { value: 'K', enabled: false },
        { value: 'L', enabled: true },
        { value: 'M', enabled: true },
        { value: 'N', enabled: true },
        { value: 'O', enabled: true },
        { value: 'P', enabled: true },
        { value: 'Q', enabled: false },
        { value: 'R', enabled: true },
        { value: 'S', enabled: true },
        { value: 'T', enabled: true },
        { value: 'U', enabled: false },
        { value: 'V', enabled: true },
        { value: 'W', enabled: false },
        { value: 'X', enabled: false },
        { value: 'Y', enabled: false },
        { value: 'Z', enabled: false }
    ];

    $scope.game = {
        user: $rootScope.user,
        gameOptions: {
            themes: [],
            availableLetters: getSelectedLetters(),
            rounds: "4",
            numberOfPlayers: "6",
            roundTime: "30"
        }
    };
    
    $scope.addTheme = (theme) => {

        if (!availableThemes.includes(theme)) {
            availableThemes.push(theme);
        }

        if ($scope.game.gameOptions.themes.includes(theme))
            return;
    };

    function getSelectedLetters() {
        let selectedLetters = [];
        $scope.letters.forEach((l) => {
            if (l.enabled) {
                selectedLetters.push(l.value);
            }
        });

        return selectedLetters;
    }

    $scope.rebuildSelectedLetters = function () {
        $scope.game.gameOptions.availableLetters = getSelectedLetters();
    };

    $scope.confirm = function() {
        $scope.game.gameOptions.availableLetters = getSelectedLetters();
        $http.post(API_SETTINGS.uri + '/api/games.create', $scope.game).then((resp) =>
        {
            $location.path('/game/' + resp.data.id);
        });
    };

    $scope.goToLobby = () => {
        $location.path('/lobby');
    };

}]);