mergeInto(LibraryManager.library, {
    LoadCDNScript: function(urlPtr, cached, callbackPtr, errorCallbackPtr) {
        var url = UTF8ToString(urlPtr);
        var callback = Module['dynCall_v'](callbackPtr);
        var errorCallback = Module['dynCall_vi'](errorCallbackPtr);

        try {
            var script = document.createElement("script");
            script.async = true;
            script.src = url;
            script.onload = function() {
                if (!cached) {
                    document.head.removeChild(script);
                }
                callback();
            };
            script.onerror = function(e) {
                errorCallback(allocateUTF8OnStack("Failed to load script: " + url));
            };
            document.head.appendChild(script);
        } catch (e) {
            errorCallback(allocateUTF8OnStack(e.message));
        }
    },

    LoadCDNModule: function(urlPtr, cached, callbackPtr, errorCallbackPtr) {
        var url = UTF8ToString(urlPtr);
        var callback = Module['dynCall_v'](callbackPtr);
        var errorCallback = Module['dynCall_vi'](errorCallbackPtr);

        try {
            var script = document.createElement("script");
            script.async = true;
            script.src = url;
            script.type = "module";
            script.onload = function() {
                if (!cached) {
                    document.head.removeChild(script);
                }
                callback();
            };
            script.onerror = function(e) {
                errorCallback(allocateUTF8OnStack("Failed to load module: " + url));
            };
            document.head.appendChild(script);
        } catch (e) {
            errorCallback(allocateUTF8OnStack(e.message));
        }
    }
});