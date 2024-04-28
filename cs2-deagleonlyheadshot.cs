using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace DeagleOnlyHeadshot;

public class DeagleOnlyHeadshot : BasePlugin
{
    public override string ModuleName => "Deagle Only Headshot";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "schwarper";

    public override void Load(bool hotReload)
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage, HookMode.Pre);
    }

    public override void Unload(bool hotReload)
    {
        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage, HookMode.Pre);
    }

    public static HookResult OnTakeDamage(DynamicHook hook)
    {
        CEntityInstance entity = hook.GetParam<CEntityInstance>(0);

        if (entity.DesignerName != "player")
        {
            return HookResult.Continue;
        }

        var info = hook.GetParam<CTakeDamageInfo>(1);

        if (info.Ability.Value?.DesignerName != "weapon_deagle")
        {
            return HookResult.Continue;
        }

        unsafe
        {
            if (GetHitGroup(hook) == HitGroup_t.HITGROUP_HEAD)
            {
                return HookResult.Continue;
            }

            hook.SetReturn(false);
            return HookResult.Handled;
        }
    }

    private static unsafe HitGroup_t GetHitGroup(DynamicHook hook)
    {
        var info = hook.GetParam<nint>(1);
        nint v4 = *(nint*)(info + 0x68);
        nint v1 = *(nint*)(v4 + 16);

        var hitgroup = HitGroup_t.HITGROUP_GENERIC;

        if (v1 != nint.Zero)
        {
            hitgroup = (HitGroup_t)(*(uint*)(v1 + 56));
        }

        return hitgroup;
    }
}
