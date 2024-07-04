// dynamicTextColor.js

// Function to set text color based on background color
function setTextColor() {
    var body = document.querySelector('body');
    var computedStyle = window.getComputedStyle(body);
    var bgColor = computedStyle.backgroundColor;
    var rgb = bgColor.match(/\d+/g);
    var brightness = (parseInt(rgb[0]) * 299 + parseInt(rgb[1]) * 587 + parseInt(rgb[2]) * 114) / 1000;

    if (brightness > 125) { // Check if background is light
        body.style.color = 'black'; // Set text color to black
    }
}

// Call setTextColor function on page load
window.onload = setTextColor;



