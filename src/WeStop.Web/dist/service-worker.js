const CACHE_NAME="swCache081120191029";let filesToCache=["/css/styles.min.css","/js/all.min.js","/index.html","/views/user/create.html"];self.addEventListener("install",function(e){e.waitUntil(caches.open(CACHE_NAME).then(function(e){return console.log("Opened cache"),e.addAll(filesToCache)}))}),self.addEventListener("activate",function(e){return console.log("[ServiceWorker] Activate"),e.waitUntil(caches.keys().then(function(e){return Promise.all(e.map(function(e){if(e!==CACHE_NAME)return console.log("[ServiceWorker] Removing old cache",e),caches.delete(e)}))})),self.clients.claim()}),self.addEventListener("fetch",function(e){"only-if-cached"===e.request.cache&&"same-origin"!==e.request.mode||e.respondWith(caches.open(CACHE_NAME).then(function(n){return n.match(e.request).then(function(n){return n||fetch(e.request).then(function(e){return e})})}))});