/*
 * Zachary Cook
 *
 * Helper functions for cookies.
 */



/*
 * Returns the cookie for a given name.
 */
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for(var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
}

/*
 * Sets the cookie for a given name.
 */
function setCookie(cookieName,value,expireTimeDays) {
    if (expireTimeDays == null) {
        expireTimeDays = 1
    }
    var date = new Date;
    date.setTime(date.getTime() + 24*60*60*1000*expireTimeDays);
    document.cookie = cookieName + "=" + value + ";path=/;expires=" + date.toGMTString();
}

/*
 * Deletes a cookie.
 */
function deleteCookie(cookieName) {
    setCookie(cookieName,"",-1)
}