const CACHE_NAME = 'swCache201020191221';

let filesToCache = [
    '/css/styles.min.css',
    '/js/all.min.js',
    '/index.html'
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