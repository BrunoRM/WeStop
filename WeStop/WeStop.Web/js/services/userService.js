angular.module('WeStop').service('$user', ['$rootScope', function($rootScope) {

    this.create = (userName) => {
        sessionStorage.setItem('user', userName);
        $rootScope.user = userName;
    }

    this.get = () => {
        return sessionStorage.getItem('user');
    }

}]);