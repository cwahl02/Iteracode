type ModalView = "login" | "register" | null;

function createModalStore() {
  let view = $state<ModalView>(null);

  return {
    get view()  { return view; },
    get open()  { return view !== null; },
    openLogin()    { view = "login"; },
    openRegister() { view = "register"; },
    close()        { view = null; },
  };
}

export const modal = createModalStore();