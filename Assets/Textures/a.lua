--
-- created with TexturePacker (http://www.codeandweb.com/texturepacker)
--
-- $TexturePacker:SmartUpdate:abc4ce4b134e6814b117452be7502ef0:819ffc1789299e6ab8485d841da6f69b:a21b8a2278a45a6f7c7a1b0206d7b142$
--
-- local sheetInfo = require("mysheet")
-- local myImageSheet = graphics.newImageSheet( "mysheet.png", sheetInfo:getSheet() )
-- local sprite = display.newSprite( myImageSheet , {frames={sheetInfo:getFrameIndex("sprite")}} )
--

local SheetInfo = {}

SheetInfo.sheet =
{
    frames = {
    
        {
            -- tiles_2
            x=2,
            y=2,
            width=560,
            height=640,

        },
    },
    
    sheetContentWidth = 564,
    sheetContentHeight = 644
}

SheetInfo.frameIndex =
{

    ["tiles_2"] = 1,
}

function SheetInfo:getSheet()
    return self.sheet;
end

function SheetInfo:getFrameIndex(name)
    return self.frameIndex[name];
end

return SheetInfo
