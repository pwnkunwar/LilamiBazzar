

async function AjaxCall(url, data) {
    var jsonData = JSON.stringify(data);
    try {
        var response = await $.ajax({
            type: "POST",
            url: url,
            timeout: 60000, // Set a reasonable timeout
            data: jsonData,
            contentType: "application/json;charset=utf-8",
            cache: false,
            async: true,
        });
        
        // Check for success
        if (response) {
            console.log("Login successful:", response);
            // Store the token or handle it as needed
            localStorage.setItem('Authorization', response);
            return response;
        } else {
            console.log("Unexpected response format:", response);
        }

    } catch (error) {
        console.error("AJAX Error:", error);
        throw error; // Optionally, rethrow or handle the error
    }
}



/*async function AjaxCall1(url, data) {
    var jsonData = JSON.stringify(data);
    try {
        var response = await $.ajax({
            type: "POST",
            url: url,
            timeout: 60000, // Set a reasonable timeout
            data: jsonData,
            contentType: "application/json;charset=utf-8",
            cache: false,
            async: true,
        });

        // Check for success
        if (response) {
            return response;
        } else {
            console.log("Unexpected response format:", response);
        }

    } catch (error) {
        console.error("AJAX Error:", error);
        throw error; // Optionally, rethrow or handle the error
    }
}*/
