actions = {}
helpcat = {}
almighty = { "splitty_", "Viperidae", "Chronium" }

-- help: help
helpcat["help"] =
    "Display this help.\r\n"

-- help: load
helpcat["load"] =
    "Load a script.\r\n\z
    Info: Loads all scripts if called without parameter.\r\n"

-- help: reload
helpcat["reload"] =
    "Reload a script.\r\n\z
    Info: Reloads all scripts if called without parameter."

-- help: unload
helpcat["unload"] =
    "Unload a script.\r\n\z
    Usage: admin unload <script>"

-- help: list
helpcat["list"] =
    "List available scripts.\r\n"

function onmessage (api, channel, message, sender)
    -- Check if the message contains spaces
    -- and begins with the plus sign
    if not
        string.find (message, " ") or
        string.sub (message, 1, 1) ~= "+" then
        return
    end
    -- Remove the first character from the message
    message = string.sub (message, 2)
    -- Split the message on every space
    local parts = {}
    for word in message:gmatch ("%w+") do
        table.insert (parts, word)
    end
    -- Check if the first part is "admin"
    if parts [1] == "admin" then
        table.remove (parts, 1)
        -- Check if the sender is privileged
        local privileged = false
        for _, v in pairs (almighty) do
            if sender == v then privileged = true end
        end
        -- Send a message and return if not
        if not privileged then
            api:call ("sendmsg", channel, "You are not privileged to do that.")
            return
        end
        local action = parts [1]
        -- Check if the action is registered
        if actions[action] ~= nil then
            table.remove (parts, 1)
            -- Call the action
            actions[action] (api, channel, message, sender, parts)
        end
    end
end

function actions.load (api, channel, message, sender, parts)
    if #parts == 0 then
        api:call ("loadscripts")
        return
    end
    script_name = parts [1]
    if reload_plugin_name ~= nil then
        api:call ("loadscript", script_name)
    end
end

function actions.reload (api, channel, message, sender, parts)
    if #parts == 0 then
        api:call ("reloadscripts")
        return
    end
    local script_name = parts [1]
    if script_name ~= nil then
        api:call ("reloadscript", script_name)
    end
end

function actions.unload (api, channel, message, sender, parts)
    local script_name = parts [1]
    if script_name == "admin" then return end
    if script_name ~= nil then
        api:call ("unloadscript", script_name)
    end
end

function actions.list (api, channel, message, sender, parts)
    local list = api:call ("listscripts")
    local accum = "Available scripts: " .. list
    api:call ("sendmsg", channel, accum)
end

function actions.help (api, channel, message, sender, parts)
    if #parts == 0 then
        local helptext = { "admin::help -> " }
        for k, v in pairs (helpcat) do
            helptext[#helptext + 1] = tostring (k)
        end
        api:call ("sendmsg", channel, table.concat (helptext, " "))
        return
    end
    local help_item = parts [1]
    if helpcat [help_item] ~= nil then
        local msg = "admin::help::" .. help_item .. " -> " .. helpcat [help_item]
        api:call ("sendmsg", channel, msg)
    end
end