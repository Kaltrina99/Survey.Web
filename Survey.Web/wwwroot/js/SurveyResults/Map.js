let formid = document.getElementById("formIdInput").value;
let serverdata;

async function getdata() {
    var response = await fetch(`/SurveyResults/Map/${formid}?data=true`);
    var data = await response.json();
    serverdata = data;
   
    mapchart();
}
function mapchart() { 
    // initialize Leaflet
    var map = L.map('map').setView({ lon: 0, lat: 0 }, 2);

    // add the OpenStreetMap tiles
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="https://openstreetmap.org/copyright">OpenStreetMap contributors</a>'
    }).addTo(map);

    // show the scale bar on the lower left corner
    L.control.scale({ imperial: true, metric: true }).addTo(map);
    var myIcon = L.divIcon({ className: 'pulse-animation' });
    for (var i = 0; i < serverdata.length; i++) {
        marker = new L.marker([parseFloat(serverdata[i].latitude), parseFloat(serverdata[i].longitude)], { icon: myIcon })
            .bindPopup(serverdata[i].title +"<br>"+ serverdata[i].endTime)
            .addTo(map);
    }
    // show a marker on the map
    
}
getdata();