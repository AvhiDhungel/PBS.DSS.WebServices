function toggleTheme(isDarkMode) {
    const themeLink = document.getElementById("theme-stylesheet");
    themeLink.href = isDarkMode ? "_content/PBS.Blazor.ClientFramework/dark-theme.css" : "_content/PBS.Blazor.ClientFramework/light-theme.css";
}

function setStyleSheet(path) {
    document.getElementById("theme-stylesheet").href = path;
}

function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

function generateICS(args) {
    var plainText = "BEGIN:VCALENDAR";
    plainText += "\r\nVERSION:2.0";
    plainText += "\r\nPRODID:-//hacksw/handcal//NONSGML v1.0//EN";
    plainText += "\r\nBEGIN:VEVENT";
    plainText += "\r\nDTSTART:" + args[0];
    plainText += "\r\nDTEND:" + args[1];
    plainText += "\r\nDTSTAMP:" + args[2];
    plainText += "\r\nSUMMARY:Service Appointment for " + args[3];
    plainText += "\r\nDESCRIPTION:" + args[4];
    plainText += "\r\nSTATUS:CONFIRMED";
    plainText += "\r\nUID:" + args[5];
    plainText += "\r\nEND:VEVENT";
    plainText += "\r\nEND:VCALENDAR";

    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/calendar;charset=utf-8,' + encodeURIComponent(plainText));
    element.setAttribute('download', 'AddtoCalendar.ics');

    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
}

function openLinkInNewTab(url) {
    window.open(url, '_blank');
}

function openPDFInNewTab(byteArray) {
    var file = new Blob([byteArray], { type: 'application/pdf' });
    var fileURL = URL.createObjectURL(file);
    if (isDevice) {
        window.open(fileURL, "_self");
    } else {
        window.open(fileURL);
    }
}

function copyTextToClipboard(text) {
    navigator.clipboard.writeText(text);
}

function requestFullScreen(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        if (element.requestFullscreen) {
            element.requestFullscreen();
        } else if (element.mozRequestFullScreen) {
            element.mozRequestFullScreen();
        } else if (element.webkitRequestFullscreen) {
            element.webkitRequestFullscreen();
        } else if (element.msRequestFullscreen) {
            element.msRequestFullscreen();
        }
    }
}