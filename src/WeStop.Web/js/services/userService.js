angular.module('WeStop').service('$user', ['$rootScope', function($rootScope) {

    this.create = (user) => {
        sessionStorage.setItem('user', JSON.stringify(user));
        $rootScope.user = user;
    }

    this.get = () => {
        return JSON.parse(sessionStorage.getItem('user'));
    }

    this.logout = (logoutCallback) => {

        sessionStorage.removeItem('user');
        $rootScope.user = null;

        if (logoutCallback)
            logoutCallback();
    }

}]);