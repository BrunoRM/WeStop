const pwaAssetGenerator = require('pwa-asset-generator');

(async () => {
    const { savedImages, htmlContent, manifestJsonContent } = await pwaAssetGenerator.generateImages(
        'E:/WeStop/WeStop/src/WeStop.Web/images/stop.png',
        'E:/WeStop/WeStop/src/WeStop.Web/icons',
        {
            scrape: false,
            background: "linear-gradient(to right, #fa709a 0%, #fee140 100%)",
            log: true
        });
})();