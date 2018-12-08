let allIdols = [
    "Ngoc Trinh",
    "Ba tung",
    "Jun Vu",
    "Hoang Thuy Linh",
    "Elly Tran",
    "Thuy Top",
    "Tam tit",
    "Midu",
    "Miu Le",
    "Chi Pu",
    "Kha Ngan",
    "Angela Phuong Trinh"

];

async function getImage(query) {
    console.log(`Begin getting images for ${query}`);
    var key = "92b91ddfaf8f4e20b5eafa76241e1f40";
    var url = `https://api.cognitive.microsoft.com/bing/v7.0/images/search?q=${query}&count=30`;

    var result = await fetch(url, {
        method: "GET",
        headers: {
            "Ocp-Apim-Subscription-Key": key
        }
    }).then(data => data.json());

    console.log(`Finish getting images for ${query}`);

    return result.value.map(x => {
        return { thumbnail: x.thumbnailUrl, image: x.contentUrl };
    });
}

function downloadJson(jsonObj) {
    const dataString = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(jsonObj));
    let anchorElem = document.createElement('a');
    anchorElem.setAttribute("href", dataString);
    anchorElem.setAttribute("download", "data.json");
    anchorElem.click();
};

(async () => {
    let idolWithImages = [];
    let index = 0;


    for (let idol of allIdols) {
        var images = await getImage(idol);
        idolWithImages.push({
            index: index++,
            name: idol,
            images: images
        });
    }

    console.log(idolWithImages);
    downloadJson(idolWithImages);
})();
