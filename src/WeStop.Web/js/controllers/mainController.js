angular.module('WeStop').controller('mainController', ['$scope', '$location', '$user', '$mdTheming', function ($scope, $location, $user, $mdTheming) {
    const themePreferenceKeyName = 'themePreference';
    $scope.preferences = {
        isDarkModeSelected: false
    };

    function checkThemePreference() {
        let themePreference = localStorage.getItem(themePreferenceKeyName);
        if (!themePreference || themePreference === '') {
            $scope.selectedTheme = $mdTheming.defaultTheme();
            storeThemePreferenceInLocalStorage();
        } else {
            $scope.selectedTheme = themePreference;
        }
    
        if ($scope.selectedTheme === 'dark') {
            $scope.preferences.isDarkModeSelected = true;
        }
    }

    $scope.logout = function () {
        $user.logout(() => $location.path('/'));
    };

    $scope.changeTheme = function() {
        if ($scope.preferences.isDarkModeSelected) {
            $scope.selectedTheme = 'dark';
        } else {
            $scope.selectedTheme = 'light';
        }

        storeThemePreferenceInLocalStorage();
    };

    function storeThemePreferenceInLocalStorage() {
        localStorage.setItem(themePreferenceKeyName, $scope.selectedTheme);
    }

    checkThemePreference();

}]);