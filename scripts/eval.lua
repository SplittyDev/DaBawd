function onmessage (api, channel, message, sender)
    if not
        string.find (message, " ") or
        string.sub (message, 1, 1) ~= "+" then
        return
    end
    message = string.sub (message, 2)
    local parts = {}
    for word in message:gmatch ("%w+") do
        table.insert (parts, word)
    end
    if parts [1] == "eval" then
        table.remove (parts, 1)
        local statements = message:sub (6)
        statements = string.format ("return (%s)", statements)
        local response = api:call ("eval", statements)
        api:call ("sendmsg", channel, response)
    end
end