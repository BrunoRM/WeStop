const CACHE_NAME = 'swCache061120190023';

let filesToCache = [
    '/node_modules/font-awesome/css/font-awesome.css',
    '/css/app.css',
    '/node_modules/angular-material/angular-material.css',
    '/node_modules/angular-material-data-table/dist/md-data-table.css',
    '/node_modules/@aspnet/signalr/dist/browser/signalr.js',
    '/node_modules/angular/angular.js',
    '/node_modules/angular-route/angular-route.js',
    '/node_modules/angular-animate/angular-animate.js',
    '/node_modules/angular-aria/angular-aria.js',
    '/node_modules/angular-messages/angular-messages.js',
    '/node_modules/angular-uuid/angular-uuid.js',
    '/node_modules/angular-material/angular-material.js',
    '/node_modules/angular-material-data-table/dist/md-data-table.js',
    '/js/app.js',
    '/js/controllers/mainController.js',
    '/js/controllers/createUserController.js',
    '/js/controllers/lobbyController.js',
    '/js/controllers/gameController.js',
    '/js/controllers/createGameController.js',
    '/js/services/gameService.js',
    '/js/services/userService.js',
    '/js/services/facebookService.js',
    '/js/services/googleService.js',
    '/js/services/twitterService.js'
];

self.addEventListener('install', function (event) {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(function (cache) {
                console.log('Opened cache');
                return cache.addAll(filesToCache);
            })
    );
});

self.addEventListener('activate', function (e) {
    console.log('[ServiceWorker] Activate');
    e.waitUntil(
        caches.keys().then(function (keyList) {
            return Promise.all(keyList.map(function (key) {
                if (key !== CACHE_NAME) {
                    console.log('[ServiceWorker] Removing old cache', key);
                    return caches.delete(key);
                }
            }));
        })
    );
    return self.clients.claim();
});

self.addEventListener('fetch', function (e) {
    // Workaround for this: https://stackoverflow.com/questions/48463483/what-causes-a-failed-to-execute-fetch-on-serviceworkerglobalscope-only-if
    if (e.request.cache === 'only-if-cached' && e.request.mode !== 'same-origin') {
        return;
    }

    e.respondWith(
        caches.open(CACHE_NAME).then(function (cache) {
            return cache.match(e.request).then(function (response) {
                return response || fetch(e.request).then(function (response) {
                    return response;
                });
            });
        })
    );
});