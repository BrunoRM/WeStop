angular.module('WeStop', [
    'ngRoute'
]);

angular.module('WeStop').config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {

    $locationProvider.hashPrefix('');

    $routeProvider

        .when('/login', {
            controller: 'loginController',
            templateUrl: 'views/login.html',
            secure: false
        })

        .when('/lobby', {
            controller: 'lobbyController',
            templateUrl: 'views/lobby.html',
            secure: true,
            resolve: {
                gameRooms: ['$http', function ($http) {

                    return $http.get('http://localhost:5000/api/gamerooms.list?onlyPublic=false').then(function (resp) {
                        return resp.data.gameRooms;
                    });

                }]
            }
        })
        
        .when('/game/:id', {
            controller: 'gameController',
            templateUrl: 'views/game.html',
            secure: true
        })
        

}]);